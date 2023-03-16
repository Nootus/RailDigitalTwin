import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { GaugesModule } from '@grptx/ng-canvas-gauges';


// Navigation
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { MovingBlockComponent } from './home/moving-block.component';
import { ADTComponent } from './home/adt.component';

// framework
import { ConfirmationDialogComponent } from './framework/confirmation-dialog.component';

// Train
import { LiveViewComponent } from './trains/live-view.component';
import { SectionComponent } from "./trains/section.component";
import { TrainListComponent } from "./trains/train-list.component";
import { RailwaySectionComponent } from "./trains/railway-section.component";
import { TrainDialogComponent } from "./trains/train-dialog.component";
import { SpeedDialogComponent } from "./trains/speed-dialog.component";

//services
import { SignalRService } from './signalr.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent, MovingBlockComponent, ADTComponent,

    ConfirmationDialogComponent,

    LiveViewComponent,
    SectionComponent,
    RailwaySectionComponent,
    TrainListComponent,
    TrainDialogComponent,
    SpeedDialogComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'live', component: LiveViewComponent },
      { path: 'moving-block', component: MovingBlockComponent },
      { path: 'adt', component: ADTComponent }
    ]),
    BrowserAnimationsModule,
    MatCardModule, MatDialogModule, MatTooltipModule, MatInputModule, MatButtonModule,
    GaugesModule
  ],
  providers: [SignalRService],
  bootstrap: [AppComponent]
})
export class AppModule { }
