import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  BehaviorSubject,
  Observable,
  delay,
  shareReplay,
  startWith,
  switchMap,
} from 'rxjs';
import {
  TodosResponse,
  TodoResponse,
  CreateTodoRequest,
  ReorderTodosRequest,
  TodoPosition,
} from '../../../api-types/api.types';

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

  public triggerRefresh(): void {
    this.refreshTrigger$.next();
  }

  public todos$: Observable<TodosResponse> = this.refreshTrigger$.pipe(
    startWith(undefined),
    switchMap(() => this.httpClient.get<TodosResponse>(this.baseUrl)),
    shareReplay(1)
  );

  /**
   * Retrieves all todos from the API.
   * @returns An Observable emitting a todosReponse object.
   */
  public fetchTodos(): Observable<TodosResponse> {
    return this.httpClient.get<TodosResponse>(this.baseUrl);
  }

  /**
   * Adds a new todo.
   * @param todoName The name of the todo to add
   * @returns Returns an Observable that emits the response from the API after creating a new todo item.
   */
  public createTodo(todoName: string): Observable<CreateTodoRequest> {
    return this.httpClient.post<CreateTodoRequest>(this.baseUrl, {
      name: todoName,
    });
  }

  public updateTodo(id: string, todo: Partial<TodoResponse>): Observable<void> {
    return this.httpClient.patch<void>(`${this.baseUrl}/${id}`, todo);
  }

  /**
   * Deletes a todo.
   * @param id The Id of the todo to delete.
   * @returns An Observable that emits the response of the API after deleting the todo.
   */
  public deleteTodo(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.baseUrl}/${id}`);
  }

  /**
   * Calls the API reorder todos endpoint.
   * @param todos An array of the reordered todos.
   * @returns Returns an Observable that emits the response of the API after reordering the todos.
   */
  public reorderTodos(todos: TodoResponse[]): Observable<ReorderTodosRequest> {
    let reorderedTodos: TodoPosition[] = todos.map((todo, index) => {
      return {
        id: todo.id,
        position: index,
      };
    });

    return this.httpClient.put<ReorderTodosRequest>(`${this.baseUrl}/reorder`, {
      todos: reorderedTodos,
    });
  }
}
