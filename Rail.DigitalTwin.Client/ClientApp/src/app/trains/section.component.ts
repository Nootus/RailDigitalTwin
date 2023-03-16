import { Component, Input } from '@angular/core';
import { SectionModel } from '../models/section.model';

@Component({
  selector: 'app-section',
  templateUrl: './section.component.html',
  styleUrls: ['./section.component.css']
})
export class SectionComponent {
  @Input() section!: SectionModel;
}
