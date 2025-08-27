import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import type { Todo } from '../../types/api/todo.model';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private httpClient = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  /**
   * Retrieves all todos from the API.
   * @returns An Observable emitting an array of Todo items.
   */
  public fetchTodos(): Observable<Todo[]> {
    return this.httpClient.get<Todo[]>(`${this.baseUrl}/todo`);
  }
}
