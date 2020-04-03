import { Component } from '@angular/core';

@Component({
  selector: 'app-run-model',
  templateUrl: './run.model.html'
})
export class RunModel {
  public currentCount = 0;

  public LoadJSON() {
    this.currentCount++;
  }
}
