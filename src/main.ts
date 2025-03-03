import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { initializeApp, provideFirebaseApp } from '@angular/fire/app';
import { environment } from './app/environment';
import { getAuth, provideAuth } from '@angular/fire/auth';
import { AngularFireModule } from '@angular/fire/compat';

bootstrapApplication(AppComponent, appConfig).catch((err) =>
  console.error(err)
);
// AngularFireModule.initializeApp(environment.firebase),
//   provideFirebaseApp(() => initializeApp(environment.firebase));
