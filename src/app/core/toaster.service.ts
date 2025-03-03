import { inject, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
@Injectable({
  providedIn: 'root',
})
export class ToasterService {
  private toastr = inject(ToastrService);

  showSuccess(msg: string) {
    this.toastr.success(`${msg}`, '', {
      closeButton: true,
      timeOut: 1000,
    });
  }
  showError(msg: string) {
    this.toastr.error(`${msg}`, '', {
      closeButton: true,
      timeOut: 2000,
    });
  }
  showWarning(msg: string) {
    this.toastr.warning(`${msg}`, '', {
      closeButton: true,
      timeOut: 2000,
    });
  }
}
