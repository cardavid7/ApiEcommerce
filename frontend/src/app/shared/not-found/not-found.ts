import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink],
  template: `
    <div class="flex flex-col items-center justify-center gap-4 py-24 text-center">
      <h1 class="text-3xl font-bold text-slate-800">404 - Página no encontrada</h1>
      <p class="text-slate-500">La página que buscas no existe.</p>
      <a routerLink="/" class="rounded-md bg-slate-800 px-4 py-2 text-white hover:bg-slate-700">Volver al inicio</a>
    </div>
  `,
})
export class NotFound {}
