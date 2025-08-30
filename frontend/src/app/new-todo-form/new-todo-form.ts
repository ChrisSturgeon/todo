import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TodoService } from '../todos/todo';

@Component({
  selector: 'app-new-todo-form',
  imports: [ReactiveFormsModule],
  templateUrl: './new-todo-form.html',
  styleUrl: './new-todo-form.css',
})
export class NewTodoForm {
  private todoService = inject(TodoService);

  public newTodoForm = new FormGroup({
    name: new FormControl(''),
  });

  public submitNewTodo() {
    const todoName = this.newTodoForm.value.name;

    if (todoName && todoName.trim().length > 0) {
      this.todoService.createTodo(todoName).subscribe(() => {
        this.todoService.triggerRefresh();
      });
    }
  }
}
