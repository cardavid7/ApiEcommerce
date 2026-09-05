import { Component } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.html',
})
export class Profile {
  constructor(readonly authService: AuthService) {}
}
