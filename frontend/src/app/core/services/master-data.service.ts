import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Item } from '../../models/item.model';
import { Location } from '../../models/location.model';

@Injectable({ providedIn: 'root' })
export class MasterDataService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = environment.apiBaseUrl;

  getItems(): Observable<Item[]> {
    return this.http.get<Item[]>(`${this.apiBaseUrl}/items`);
  }

  getLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(`${this.apiBaseUrl}/locations`);
  }
}
