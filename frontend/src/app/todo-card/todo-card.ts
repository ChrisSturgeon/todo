import { Component, inject, input } from '@angular/core';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { DeleteButton } from '../delete-button/delete-button';
import { TodoService } from '../todo-service/todo.service';
import type { TodoResponse } from '../../../api-types/api.types';

@Component({
  selector: 'app-todo-card',
  imports: [CdkDrag, DeleteButton],
  templateUrl: './todo-card.html',
  styleUrl: './todo-card.css',
})
export class TodoCard {
  private todoService = inject(TodoService);
  public todo = input.required<TodoResponse>();

  public toggleDone() {
    this.todo().completed = !this.todo().completed;
    this.todoService.updateTodo(this.todo().id, this.todo()).subscribe({
      next: () => {},
      error: (err) => {
        this.todo().completed = !this.todo().completed;
        console.error('Error updating todo: ', err);
      },
    });
  }
}
