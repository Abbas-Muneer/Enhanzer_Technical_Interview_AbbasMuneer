import { Location } from './location.model';

export interface LoginResponse {
  success: boolean;
  message: string;
  locations: Location[];
}
