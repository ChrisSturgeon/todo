import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import type { Todo } from '../../types/api/todo.model';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/todo`;

  private refreshTrigger$ = new BehaviorSubject<void>(undefined);

  get refresh$() {
    return this.refreshTrigger$.asObservable();
  }

  public triggerRefresh() {
    this.refreshTrigger$.next();
  }

  /**
   * Retrieves all todos from the API.
   * @returns An Observable emitting an array of Todo items.
   */
  public fetchTodos(): Observable<Todo[]> {
    return this.httpClient.get<Todo[]>(this.baseUrl);
  }

  /**
   * Adds a new todo.
   * @param todoName The name of the todo to add
   * @returns Returns an Observable that emits the response from the API after creating a new todo item.
   */
  public createTodo(todoName: string): Observable<Todo> {
    return this.httpClient.post<Todo>(this.baseUrl, { name: todoName });
  }
}
