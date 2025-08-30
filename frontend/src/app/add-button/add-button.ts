import { Component, inject } from '@angular/core';
import { TodoService } from '../todos/todo';

@Component({
  selector: 'app-add-button',
  imports: [],
  templateUrl: './add-button.html',
  styleUrl: './add-button.css',
})
export class AddButton {
  private todoService: TodoService = inject(TodoService);

  public addRandomTodo() {
    this.todoService.createTodo(Date.now().toString()).subscribe(() => {
      this.todoService.triggerRefresh();
    });
  }
}
