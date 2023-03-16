import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-train-dialog',
  templateUrl: './train-dialog.component.html',
})
export class TrainDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<TrainDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
  }
}
