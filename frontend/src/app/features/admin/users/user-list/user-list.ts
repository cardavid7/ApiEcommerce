import { Component, OnInit, signal } from '@angular/core';
import { UserService } from '../../../../core/services/user.service';
import { UserData } from '../../../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.html',
})
export class UserList implements OnInit {
  readonly users = signal<UserData[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor(private readonly userService: UserService) {}

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los usuarios.');
        this.isLoading.set(false);
      },
    });
  }
}
