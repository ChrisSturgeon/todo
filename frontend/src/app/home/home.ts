import { Component } from '@angular/core';
import { NewTodoForm } from '../features/todo/components/new-todo-form/new-todo-form';
import { TodoList } from '../features/todo/components/todo-list/todo-list';

@Component({
  selector: 'app-home',
  imports: [NewTodoForm, TodoList],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {}
