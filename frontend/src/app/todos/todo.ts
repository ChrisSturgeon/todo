import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Observable, delay } from 'rxjs';
import type { Todo } from '../../types/api/todo.model';
import { ReorderTodoDto } from '../../types/api/reorderTodoDto.model';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/todo`;

  private refreshTrigger$ = new BehaviorSubject<void>(undefined);

  public get refresh$() {
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

  /**
   * Deletes a todo.
   * @param id The Id of the todo to delete.
   * @returns An Observable that emits the response of the API after deleting the todo.
   */
  public deleteTodo(id: string): Observable<Todo> {
    return this.httpClient.delete<Todo>(`${this.baseUrl}/${id}`);
  }

  /**
   * Calls the API reorder todos endpoint.
   * @param todos An array of the reordered todos.
   * @returns Returns an Observable that emits the response of the API after reordering the todos.
   */
  public reorderTodos(todos: Todo[]): Observable<Todo[]> {
    let reorderedTodos: ReorderTodoDto[] = todos.map((todo, index) => {
      return {
        id: todo.id,
        position: index,
      };
    });

    return this.httpClient.put<Todo[]>(
      `${this.baseUrl}/reorder`,
      reorderedTodos
    );
  }
}
