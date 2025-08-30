import { Component, inject, input } from '@angular/core';
import { TodoService } from '../todos/todo';

@Component({
  selector: 'app-delete-button',
  imports: [],
  templateUrl: './delete-button.html',
  styleUrl: './delete-button.css',
})
export class DeleteButton {
  private todoService = inject(TodoService);

  public todoId = input.required<string>();

  public deleteTodo() {
    this.todoService.deleteTodo(this.todoId()).subscribe(() => {
      this.todoService.triggerRefresh();
    });
  }
}
