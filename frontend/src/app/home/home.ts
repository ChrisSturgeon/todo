import { Component } from '@angular/core';
import { NewTodoForm } from '../new-todo-form/new-todo-form';
import { TodoList } from '../todo-list/todo-list';

@Component({
  selector: 'app-home',
  imports: [NewTodoForm, TodoList],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {}
