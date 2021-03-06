import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { FooterComponent, NavigationComponent, NotFoundComponent } from './shared';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    NavigationComponent,
    NotFoundComponent
  ],
  imports: [
    BrowserAnimationsModule,
    CoreModule,
    SharedModule,

    AppRoutingModule // must be imported as the last module as it contains the fallback route
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
