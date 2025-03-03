import { HttpInterceptorFn } from '@angular/common/http';

export const interceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const accessToken = localStorage.getItem('accessToken');
  let loggedUserData: any;

  if (accessToken != null) {
    loggedUserData = accessToken;
  }
  console.log('loggedUserData', loggedUserData);

  const cloneRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${loggedUserData}`,
    },
  });
  return next(cloneRequest);
};
