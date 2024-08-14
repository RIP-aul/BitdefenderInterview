import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  public title = 'Angular Client';

  public selectedOption = 0;

  public pauseOptions = [
    0,
    1,
    5,
    10,
    15,
    30,
    60
  ]

  constructor(
    private apiService: ApiService) {
    }

  public startOnDemandScan(): void {
    this.apiService.startOnDemandScan().subscribe({
      next() {
        console.log();
      },
      error(err) {
        console.error('Something went wrong: ' + err);
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public stopOnDemandScan(): void {
    this.apiService.stopOnDemandScan().subscribe({
      next() {
        console.log();
      },
      error(err) {
        console.error('Something went wrong: ' + err);
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public activateRealTimeScan(): void {
    this.apiService.activateRealTimeScan().subscribe({
      next() {
        console.log();
      },
      error(err) {
        console.error('Something went wrong: ' + err);
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public deactivateRealTimeScan(): void {
    this.apiService.deactivateRealTimeScan(this.selectedOption).subscribe({
      next() {
        console.log();
      },
      error(err) {
        console.error('Something went wrong: ' + err);
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public getEventLog(): void {
    this.apiService.getEventLogs().subscribe({
      next() {
        console.log();
      },
      error(err) {
        console.error('Something went wrong: ' + err);
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public setPauseOption(option: any): void {
    this.selectedOption = option.value;
  }
}
