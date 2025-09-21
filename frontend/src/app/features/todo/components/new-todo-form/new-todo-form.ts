import { Component, inject } from '@angular/core';
import { TodoService } from '../../services/todo-service/todo.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-new-todo-form',
  imports: [ReactiveFormsModule],
  templateUrl: './new-todo-form.html',
  styleUrl: './new-todo-form.css',
})
export class NewTodoForm {
  private todoService = inject(TodoService);

  public newTodoForm = new FormGroup({
    name: new FormControl('', [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(50),
    ]),
  });

  public submitNewTodo() {
    const todoName = this.newTodoForm.value.name;

    if (this.newTodoForm.valid) {
      this.todoService.createTodo(todoName!).subscribe(() => {
        this.todoService.triggerRefresh();
        this.newTodoForm.reset();
      });
    }
  }

  get name() {
    return this.newTodoForm.get('name');
  }
}
