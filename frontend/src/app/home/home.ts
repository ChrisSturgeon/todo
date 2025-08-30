import { Todo } from '../../types/api/todo.model';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { TodoService } from '../todo-service/todo.service';
import { Subscription, startWith, switchMap } from 'rxjs';
import { Draggable } from '../draggable/draggable';
import {
  CdkDragDrop,
  CdkDropList,
  moveItemInArray,
} from '@angular/cdk/drag-drop';
import { AddButton } from '../add-button/add-button';
import { NewTodoForm } from '../new-todo-form/new-todo-form';

@Component({
  selector: 'app-home',
  imports: [Draggable, CdkDropList, NewTodoForm],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy {
  private todoService: TodoService = inject(TodoService);
  private subscription = new Subscription();

  public isLoading = false;
  public isError = false;
  public todos: Todo[] = [];

  ngOnInit(): void {
    this.isLoading = true;
    this.subscription.add(
      this.todoService.todos$.subscribe({
        next: (todos) => {
          this.todos = todos;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching todos:', error);
          this.isError = true;
          this.isLoading = false;
        },
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public drop(event: CdkDragDrop<Todo[]>) {
    moveItemInArray(this.todos, event.previousIndex, event.currentIndex);

    this.todoService.reorderTodos(this.todos).subscribe(() => {
      this.todoService.triggerRefresh();
      this.isLoading = true;
    });
  }
}
