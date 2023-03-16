import { Component, Inject, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TrainModel } from '../models/train.model';

@Component({
  selector: 'app-speed-dialog',
  templateUrl: './speed-dialog.component.html',
})
export class SpeedDialogComponent {
  @ViewChild('form') form!: NgForm;

  constructor(
    public dialogRef: MatDialogRef<SpeedDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public train: TrainModel) {
  }
}
