import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MovieService {
  constructor(private http: HttpClient) {}
  getAllMovie(): Observable<any> {
    return this.http.get('https://localhost:7059/api/Movie/getAllMovieData');
  }
  searchMovie(query: string, apikey: string, UserId: number): Observable<any> {
    return this.http.get(
      `https://localhost:7059/api/Movie/getAllMovie?s=${query}&apikey=${apikey}&UserId=${UserId}`
    );
  }
  addMovie(movie: FormData): Observable<any> {
    return this.http.post('https://localhost:7059/api/Movie/addMovie', movie);
  }
  updateMovie(movie: FormData): Observable<any> {
    console.log('Movie:', movie);

    return this.http.put('https://localhost:7059/api/Movie/updateMovie', movie);
  }
  deleteMovie(movieId: number): Observable<any> {
    return this.http.delete(
      `https://localhost:7059/api/Movie/deleteMovie?movieId=${movieId}`
    );
  }
  getMoviesById(userId: number): Observable<any> {
    return this.http.get(
      `https://localhost:7059/api/Movie/AllMovieBydId/${userId}`
    );
  }
}
