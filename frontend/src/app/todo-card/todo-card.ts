import { Component, inject, input } from '@angular/core';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { DeleteButton } from '../delete-button/delete-button';
import { Todo } from '../../types/api/todo.model';
import { TodoService } from '../todo-service/todo.service';

@Component({
  selector: 'app-todo-card',
  imports: [CdkDrag, DeleteButton],
  templateUrl: './todo-card.html',
  styleUrl: './todo-card.css',
})
export class TodoCard {
  private todoService = inject(TodoService);
  public todo = input.required<Todo>();

  public toggleDone() {
    console.log('todo was completed', this.todo().completed);
    this.todo().completed = !this.todo().completed;
    console.log('todo is now: ', this.todo().completed);

    this.todoService.updateTodo(this.todo().id, this.todo()).subscribe({
      next: () => {
        console.log('Todo updated');
      },
      error: (err) => {
        console.error('Error updating todo: ', err);
      },
    });
  }
}
