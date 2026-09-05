import { DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../core/models/category.model';

@Component({
  selector: 'app-category-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './category-list.html',
})
export class CategoryList implements OnInit {
  readonly categories = signal<Category[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor(private readonly categoryService: CategoryService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.categoryService.getCategories().subscribe({
      next: (categories) => {
        this.categories.set(categories);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar las categorías.');
        this.isLoading.set(false);
      },
    });
  }

  remove(category: Category): void {
    if (!confirm(`¿Eliminar la categoría "${category.name}"?`)) {
      return;
    }
    this.categoryService.deleteCategory(category.id).subscribe({
      next: () => this.load(),
      error: () => this.errorMessage.set('No se pudo eliminar la categoría (puede tener productos asociados).'),
    });
  }
}
