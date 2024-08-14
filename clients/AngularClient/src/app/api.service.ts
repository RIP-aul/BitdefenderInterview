import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})

export class ApiService {
    constructor(
        private httpClient: HttpClient) {}

    public startOnDemandScan(): Observable<any> {
        return this.httpClient.get('https://localhost:44377/on-demand-scan/start')
    }

    public stopOnDemandScan(): Observable<any> {
        return this.httpClient.get('https://localhost:44377/on-demand-scan/stop')
    }

    public activateRealTimeScan(): Observable<any> {
        return this.httpClient.get('https://localhost:44377/real-time-scan/activate')
    }

    public deactivateRealTimeScan(pauseOption: number): Observable<any> {
        return this.httpClient.get(`https://localhost:44377/real-time-scan/deactivate/${pauseOption}`)
    }

    public getEventLogs(): Observable<any>{
        return this.httpClient.get('https://localhost:44377/event-log')
    }
}