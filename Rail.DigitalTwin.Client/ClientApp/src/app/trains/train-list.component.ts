import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { SpeedDialogComponent } from "./speed-dialog.component";
import { TrainModel } from '../models/train.model';
import { RadialGaugeOptions } from '@grptx/ng-canvas-gauges';

@Component({
  selector: 'app-train-list',
  templateUrl: './train-list.component.html',
  styleUrls: ['./train-list.component.css']
})
export class TrainListComponent {
  @Input() trains!: TrainModel[];
  @Output() adjustSpeedEvent = new EventEmitter<TrainModel>();

  radial_options = <RadialGaugeOptions>{
    width: 220,
    height: 220,
    title: "Km/h",
    minValue: 0,
    maxValue: 200,
    startAngle: 90,
    ticksAngle: 180,

    valueBox: false,


    majorTicks: [
      "0", "20", "40", "60", "80", "100", "120", "140", "160", "180", "200"
    ],
    minorTicks: 2,
    strokeTicks: true,
    highlights: [
      {
        "from": 150,
        "to": 200,
        "color": "rgba(200, 50, 50, .75)"
      }
    ],
    borderShadowWidth: 0,
    borders: false,
    needleType: "arrow",
    needleWidth: 2,
    animationDuration: 1500,
    animationRule: "linear",

    colorPlate: "#eee",
  };

  constructor(public dialog: MatDialog) { }

  adjustSpeed(train: TrainModel): void {
    const dialogRef = this.dialog.open(SpeedDialogComponent, {
      width: "250px",
      data: train
    });

    // Get the result from the dialog after it is closed
    dialogRef.afterClosed().subscribe((result) => {
      if (result !== undefined)
        this.adjustSpeedEvent.emit(result);
    });
  }
}
