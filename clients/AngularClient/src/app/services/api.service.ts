import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})

export class ApiService {
    public readonly envUrl = "http://localhost:5072"
    
    constructor(
        private httpClient: HttpClient) {}

    public startOnDemandScan(): Observable<any> {
        return this.httpClient.post(`${this.envUrl}/on-demand-scan/start`, {})
    }

    public stopOnDemandScan(): Observable<any> {
        return this.httpClient.post(`${this.envUrl}/on-demand-scan/stop`, {})
    }

    public activateRealTimeScan(): Observable<any> {
        return this.httpClient.post(`${this.envUrl}/real-time-scan/activate`, {})
    }

    public deactivateRealTimeScan(pauseOption: number): Observable<any> {
        return this.httpClient.post(`${this.envUrl}/real-time-scan/deactivate/${pauseOption}`, {})
    }

    public getEventLogs(): Observable<any>{
        return this.httpClient.get(`${this.envUrl}/event-log`)
    }
}