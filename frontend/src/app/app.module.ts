import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { MaterialModule } from './modules/material/material.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    MaterialModule,
    CoreModule,
    SharedModule,

    AppRoutingModule // must be imported as the last module as it contains the fallback route
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
