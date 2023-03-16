import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { SectionModel } from '../models/section.model';
import { TrainModel } from '../models/train.model';
import { globalVariable } from '../models/global-variable';

@Component({
  selector: 'app-railway-section',
  templateUrl: './railway-section.component.html',
  styleUrls: ['./railway-section.component.css']
})
export class RailwaySectionComponent implements OnChanges {
  @Input() section!: SectionModel;
  @Input() trains!: TrainModel[];

  sectionPixels: number = 1400;
  pixelPerMeter: number = 0;
  criticalPixles: number = 0;
  safePixles: number = 0;

  ngOnChanges(changes: SimpleChanges) {
    if (changes.section) {
      this.pixelPerMeter = this.sectionPixels / (this.section.length * 1000)
      this.safePixles = this.pixelPerMeter * this.section.safeDistance;
      this.criticalPixles = this.pixelPerMeter * this.section.criticalDistance;
    }

    if (changes.trains && this.trains.length > 0) {
      this.trains.forEach(train => {
        train.trainWidth = Math.round(this.pixelPerMeter * train.trainLength);
        let offset = globalVariable.mainOffsetLeft - this.criticalPixles - this.safePixles;
        train.trainLocation = Math.round(train.frontTravelled * this.pixelPerMeter) - train.trainWidth + offset;
      })
    }
  }
}
