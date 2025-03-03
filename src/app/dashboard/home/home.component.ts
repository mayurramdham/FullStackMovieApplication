import {
  AfterViewInit,
  Component,
  inject,
  OnInit,
  PipeTransform,
  ViewChild,
} from '@angular/core';
import { MovieService } from '../../core/movie.service';
import { CommonModule } from '@angular/common';
import { JwtServiceService } from '../../core/jwt-service.service';
import {
  MatPaginator,
  MatPaginatorIntl,
  MatPaginatorModule,
} from '@angular/material/paginator';
import Swal from 'sweetalert2';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { SafeUrlPipe } from '../../safe-url.pipe';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from '../../msal-Config';
import { getAuth, signOut } from '@angular/fire/auth';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule, MatPaginatorModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit, AfterViewInit {
  private movieService = inject(MovieService);
  msalInstance!: PublicClientApplication;

  query: string = '';
  searchResults: any[] = [];
  paginatedMovies: any[] = [];
  private jwtService = inject(JwtServiceService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toasterService = inject(ToastrService);
  @ViewChild('paginator', { static: true }) paginator!: MatPaginator;
  //dataSource = new MatTableDataSource<ApidatumDisplay>([]);
  displayedColumns!: string[];
  totalRecords = 0;
  pageSize = 4;
  pageIndex = 0;
  userId: number = this.jwtService.getUserId();

  getPagedData() {
    const start = this.pageIndex * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedMovies = this.searchResults?.slice(start, end);
    console.log('paginatedmovies', this.paginatedMovies);
  }
  pageChangeEvent(event: any) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getPagedData();
  }
  ngAfterViewInit() {
    this.paginator = this.paginator;
  }

  apiKey: string = this.jwtService.getApiKey(); // Replace with your actual API key
  movieForm!: FormGroup;
  currentRole: string = this.jwtService.getRole();
  selectedFile: File | null = null;
  constructor(private fb: FormBuilder) {
    this.movieForm = this.fb.group({
      movieId: [''],
      movieTitle: ['', Validators.required],
      releaseYear: ['', [Validators.required]],
      movieLink: ['', Validators.required],
    });
    this.msalInstance = new PublicClientApplication(msalConfig);
  }

  validateReleaseYear(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    if (value.length > 4) {
      input.value = value.slice(0, 4);
    }
  }
  resetForm() {
    this.movieForm.reset();
  }
  onFileSelectUpdate(event: any) {
    const file = event.target.files[0];
    this.movieForm.patchValue({ posterImage: file });
  }
  openModal(movie: any) {
    console.log('Movie:', movie);

    // Set the current movie being edited
    this.movieForm.patchValue({
      movieId: movie.movieId,
      movieTitle: movie.movieTitle,
      releaseYear: movie.releaseYear,
      movieLink: movie.movieLink,
      posterImage: null, // You can handle file input separately
    });

    const modal = document.getElementById('updateMovieModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  closeModal() {
    const modal = document.getElementById('updateMovieModal');
    if (modal) {
      modal.style.display = 'none';
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
      this.resetForm();
    }
  }

  submitUpdateMovie() {
    if (this.movieForm.valid) {
      const updatedMovie = this.movieForm.value;
      const formData = new FormData();
      formData.append('movieId', updatedMovie.movieId);
      formData.append('movieTitle', updatedMovie.movieTitle);
      formData.append('releaseYear', updatedMovie.releaseYear);
      formData.append('movieLink', updatedMovie.movieLink);
      // formData.append('posterImage', this.selectedFile);
      if (this.selectedFile) {
        formData.append('posterImage', this.selectedFile);
      }
      formData.forEach((value, key) => {
        console.log(`${key}:`, value);
      });

      console.log('Updated Movie:', updatedMovie);
      // Handle API call to update the movie here
      this.movieService.updateMovie(formData).subscribe({
        next: (response: any) => {
          if (response.status == 200) {
            this.toasterService.success(response.message);
            this.selectedFile = null;
            this.closeModal();
            this.resetForm();
            this.getAllMoviesById(this.userId);
          } else if (response.status == 404) {
            this.toasterService.error(response.message);
          } else {
            this.toasterService.error(
              'An unexpected error occurred. Please try again.'
            );
          }
        },
        error: (error) => {
          console.error(error.message);
        },
      });

      // Close the modal after successful update
    }
  }

  onSearch(): void {
    if (this.query.length === 0 || this.query.length === 1) {
      this.getAllMovies();
      this.router.navigate(['/dashboard/home']);
    } else if (this.query.length >= 1) {
      this.movieService
        .searchMovie(this.query, this.apiKey, this.userId)
        .subscribe(
          (results) => {
            this.paginatedMovies = results.data;
            console.log('serachresult', this.searchResults);

            this.router.navigate([], {
              relativeTo: this.route,
              queryParams: { s: this.query, apikey: this.apiKey },
              queryParamsHandling: 'merge',
            });
          },
          (error) => {
            console.error('Error fetching movie data', error);
          }
        );
    } else {
      this.searchResults = [];
    }
  }

  movies: any[] = [];

  ngOnInit(): void {
    this.currentRole === 'Admin'
      ? this.getAllMoviesById(this.userId)
      : this.getAllMovies();
  }

  getAllMovies() {
    this.movieService.getAllMovie().subscribe({
      next: (response: any) => {
        if (response.status == 200) {
          this.searchResults = response.data;
          console.log('moviesdata', this.searchResults);
          this.getPagedData();
        }
      },
      error: (response) => {
        console.error(response);
      },
    });
  }

  getAllMoviesById(userId: number) {
    console.log('userId for moviess', userId);
    this.movieService.getMoviesById(userId).subscribe({
      next: (response) => {
        console.log(response);
        if (response.status == 200) {
          console.log('response', response.movieData);
          this.searchResults = response.movieData;
          this.getPagedData();
          console.log('moviesDataAdmin', this.paginatedMovies);
        }
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  onEditMovie(hhdh: any) {
    // Implement edit movie functionality
  }

  onDeleteMovie(movieId: any) {
    console.log('Delete movie clicked:', movieId);
    Swal.fire({
      title: 'Are you sure you want to delete this movie?',
      showDenyButton: true,
      showCancelButton: true,
      confirmButtonText: 'Delete',
      denyButtonText: `Don't delete`,
    }).then((result) => {
      if (result.isConfirmed) {
        this.movieService.deleteMovie(movieId).subscribe({
          next: (response) => {
            if (response.status == 200) {
              Swal.fire('Deleted!', '', 'success');
              this.getAllMoviesById(this.userId);
            } else {
              Swal.fire('Failed to delete', '', 'error');
            }
          },
          error: (error) => {
            Swal.fire(error.message, '', 'error');
          },
        });
      } else if (result.isDenied) {
        Swal.fire('Movie not deleted', '', 'info');
      }
    });
  }

  onFileSelect(event: Event): void {
    console.log('event', event);

    const input = event.target as HTMLInputElement;
    if (input?.files?.[0]) {
      this.selectedFile = input.files[0];
    }
  }

  // submitMovie(): void {
  //   if (this.movieForm.valid && this.selectedFile) {
  //     const formData = new FormData();
  //     formData.append('MovieTitle', this.movieForm.get('movieTitle')?.value);
  //     formData.append('ReleaseYear', this.movieForm.get('releaseYear')?.value);
  //     formData.append('posterImage', this.selectedFile);

  //     formData.forEach((value, key) => {
  //       console.log(`${key}:`, value);
  //     });

  //     this.movieService.addMovie(formData).subscribe({
  //       next: (response: any) => {
  //         console.log('addRequest', response);
  //         if (response.status === 200) {
  //           this.toasterService.success(response.message);
  //           this.selectedFile == null;
  //           this.closeModal();

  //           this.getAllMovies();
  //         } else if (response.status === 404) {
  //           this.toasterService.error(response.message);
  //         } else if (response.status === 400) {
  //           this.toasterService.error(response.message);
  //         } else {
  //           this.toasterService.error(response.message);
  //         }
  //       },
  //       error: (error) => {
  //         console.log('error message', error);
  //         this.toasterService.error(error.errors.error);
  //       },
  //     });
  //     //  Perform HTTP request to backend
  //     // this.http.post('/api/movies', formData).subscribe(...);
  //   } else {
  //     alert('Please fill in all fields and upload a file.');
  //   }
  // }

  // submitMovie(): void {
  //   console.log('submitMovie called'); // Add this to check if the function is triggered
  //   if (this.movieForm.valid && this.selectedFile) {
  //     const formData = new FormData();
  //     formData.append('MovieTitle', this.movieForm.get('movieTitle')?.value);
  //     formData.append('ReleaseYear', this.movieForm.get('releaseYear')?.value);
  //     formData.append('posterImage', this.selectedFile);

  //     formData.forEach((value, key) => {
  //       console.log(`${key}:`, value);
  //     });

  //     this.movieService.addMovie(formData).subscribe({
  //       next: (response: any) => {
  //         console.log('addRequest', response);
  //         if (response.status == 200) {
  //           this.toasterService.success(response.message);
  //           this.closeModal();
  //           this.selectedFile = null;
  //           this.getAllMovies();
  //         } else {
  //           this.toasterService.error(response.message || 'Unknown error');
  //         }
  //       },
  //       error: (error) => {
  //         console.log('error message', error);
  //         this.toasterService.error(
  //           error.errors.error.toString() || 'Something went wrong'
  //         );
  //       },
  //     });
  //   } else {
  //     alert('Please fill in all fields and upload a file.');
  //   }
  // }

  submitMovie(): void {
    console.log('submitMovie called'); // Check if the function is triggered

    if (this.movieForm.valid && this.selectedFile) {
      // Create a simple payload without the file, just the form data
      const payload = {
        MovieTitle: this.movieForm.get('movieTitle')?.value,
        ReleaseYear: this.movieForm.get('releaseYear')?.value,
        movieLink: this.movieForm.get('movieLink')?.value,
        Id: Number(this.userId),
      };

      console.log('Payload:', payload); // Log to see the payload

      // Create FormData to send the file separately
      const formData = new FormData();

      formData.append('MovieTitle', payload.MovieTitle); // You can still append the other data to FormData
      formData.append('ReleaseYear', payload.ReleaseYear);
      formData.append('posterImage', this.selectedFile);
      formData.append('movieLink', payload.movieLink);
      formData.append('Id', this.jwtService.getUserId()); // Attach the file

      // Send the payload to the service
      this.movieService.addMovie(formData).subscribe({
        next: (response: any) => {
          console.log('addRequest', response);
          if (response.status === 200) {
            this.toasterService.success(response.message);
            this.closeModal();
            this.selectedFile = null;
            this.getAllMoviesById(this.userId);
          } else {
            this.toasterService.error(response.message || 'Unknown error');
          }
        },
        error: (error) => {
          console.log('error message', error);
          this.toasterService.error(
            error.error.errors.toString() || 'Something went wrong'
          );
        },
      });
    } else {
      alert('Please fill in all fields and upload a file.');
    }
  }

  LogOut() {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You are about to log out from the application!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, log out!',
    }).then((result) => {
      if (result.isConfirmed) {
        // Clear local storage first
        localStorage.clear();
        this.router.navigate(['/auth/login']);
        // Sign out from Firebase first
        const auth = getAuth();
        signOut(auth)
          .then(() => {
            // Optionally show a success message before Microsoft logout
            Swal.fire({
              title: 'Logged out!',
              text: 'You have logged out from Firebase.',
              icon: 'success',
              timer: 1500,
              showConfirmButton: false,
            }).then(() => {
              // Then log out from Microsoft
              this.msalInstance.logoutRedirect();
            });
          })
          .catch((error) => {
            console.error('Firebase logout error:', error);
            Swal.fire({
              title: 'Error',
              text: error.message,
              icon: 'error',
            }).then(() => {
              // Even if Firebase sign-out fails, attempt Microsoft logout
              this.msalInstance.logoutRedirect();
            });
          });
      }
    });
  }

  selectedTrailerUrl: SafeResourceUrl | null = null;
  private sanitizer = inject(DomSanitizer);

  private convertToEmbedUrl(url: string): string {
    const regExp =
      /(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=))([^&\n?#]+)/;
    const match = url.match(regExp);
    console.log('match', match);
    return match ? `https://www.youtube.com/embed/${match[1]}` : url;
  }

  playTrailer(url: string | undefined): void {
    if (!url) {
      this.selectedTrailerUrl = null;
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Trailer for this movie is not available!',
      });
      return;
    }
    // Convert and sanitize the URL (if needed)
    const embedUrl = this.convertToEmbedUrl(url);
    this.selectedTrailerUrl =
      this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl);
  }
  closeTrailer(): void {
    this.selectedTrailerUrl = null;
  }
}
