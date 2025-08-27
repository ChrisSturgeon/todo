import { Component, inject } from '@angular/core';
import { Todo } from '../../types/api/todo.model';
import { TodoService } from '../todos/todo';
import { Observable, Subscription } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private todoService: TodoService = inject(TodoService);
  public todos$: Observable<Todo[]>;

  constructor() {
    this.todos$ = this.todoService.fetchTodos();
  }
}
