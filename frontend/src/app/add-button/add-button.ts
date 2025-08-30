import { Component, inject } from '@angular/core';
import { TodoService } from '../todo-service/todo.service';
import { Observable } from 'rxjs';
import { Todo } from '../../types/api/todo.model';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-add-button',
  imports: [AsyncPipe],
  templateUrl: './add-button.html',
  styleUrl: './add-button.css',
})
export class AddButton {
  private todoService: TodoService = inject(TodoService);
  public todosList$: Observable<Todo[]>;

  constructor() {
    this.todosList$ = this.todoService.todos$;
  }
}
