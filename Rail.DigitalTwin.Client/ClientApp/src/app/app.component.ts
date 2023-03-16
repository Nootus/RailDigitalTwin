import { Component, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { globalVariable } from './models/global-variable';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements AfterViewInit {
  @ViewChild('main') mainElementRef!: ElementRef;

  title = "Moving Block Digital Twin";

  ngAfterViewInit() {
    globalVariable.mainOffsetLeft = this.mainElementRef.nativeElement.offsetLeft;
  //  console.log(this.main.nativeElement); // Hello world
  //  console.log(this.main.nativeElement.offsetLeft); // 10
  //  console.log(this.main.nativeElement.style.marginLeft); // 10px
  }
}
