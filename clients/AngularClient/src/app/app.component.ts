import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './services/api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button'
import { MatSelectModule } from '@angular/material/select'
import { MatSnackBar } from '@angular/material/snack-bar'
import * as _ from 'lodash'

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, FormsModule, MatButtonModule, MatSelectModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  public title = 'Angular Client';
  public eventLog: Array<EventLog> = [];
  public selectedOption = 0;

  public pauseOptions = [
    1,
    5,
    10,
    15,
    30,
    60
  ]

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar) {
    }
 
  public startOnDemandScan(): void {
    this.apiService.startOnDemandScan().subscribe({
      next: (response: any) => {
        this.snackBar.open(response.message, 'Close' , {
          duration: 5000, // Automatically dismiss after 5 seconds
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      }, 
      error: (err: any) => {
        this.snackBar.open(err.error.message, 'Okay', {
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      complete: () => console.log('Done')
    });
  }

  public stopOnDemandScan(): void {
    this.apiService.stopOnDemandScan().subscribe({
      next: (response: any) => {
        this.snackBar.open(response.message, 'Close' , {
          duration: 5000, // Automatically dismiss after 5 seconds
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      error: (err: any) => {
        this.snackBar.open(err.error.message, 'Okay', {
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public activateRealTimeScan(): void {
    this.apiService.activateRealTimeScan().subscribe({
      next: (response: any) => {
        this.snackBar.open(response.message, 'Close' , {
          duration: 5000, // Automatically dismiss after 5 seconds
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      }, 
      error: (err: any) => {
        this.snackBar.open(err.error.message, 'Okay', {
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      complete: () => console.log('Done')
    });
  }

  public deactivateRealTimeScan(): void {
    this.apiService.deactivateRealTimeScan(this.selectedOption).subscribe({
      next: (response: any) => {
        this.snackBar.open(response.message, 'Close' , {
          duration: 5000, // Automatically dismiss after 5 seconds
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      error: (err: any) => {
        this.snackBar.open(err.error.message, 'Okay', {
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      complete() {
        console.log('Done');
      }
    });
  }

  public getEventLog(): void {
    this.apiService.getEventLogs().subscribe({
      next: (value: Array<EventLogDto>) =>
        this.eventLog = (_.sortBy(value, 'timeOfEvent').reverse()).map(item =>
          ({
            timeOfEvent: new Date(item.timeOfEvent), // make date more user friendly
            newStatus: item.newStatus,
            oldStatus: item.oldStatus,
            antivirusDetectionResult: item.antivirusDetectionResult
          })),
      error: (err: any) => {
        this.snackBar.open(err.error.message, 'Okay', {
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        })
      },
      complete: () => console.log('Done')
    });
  }

  public setPauseOption(option: any): void {
    this.selectedOption = option.value;
  }
}

export type EventLogDto = {
  timeOfEvent: string;
  newStatus?: string;
  oldStatus?: string;
  antivirusDetectionResult?: Threat;
}

export type EventLog = {
  timeOfEvent: Date;
  newStatus?: string;
  oldStatus?: string;
  antivirusDetectionResult?: Threat;
}

export type Threat = {
  path: string;
  threatName: string;
}