import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { TodoService } from '../todo-service/todo.service';
import { Subscription } from 'rxjs';
import {
  CdkDragDrop,
  moveItemInArray,
  CdkDropList,
} from '@angular/cdk/drag-drop';
import type { Todo } from '../../types/api/todo.model';
import { TodoCard } from '../todo-card/todo-card';

@Component({
  selector: 'app-todo-list',
  imports: [CdkDropList, TodoCard],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.css',
})
export class TodoList implements OnInit, OnDestroy {
  private todoService = inject(TodoService);
  private subscription = new Subscription();

  public todos: Todo[] = [];
  public isLoading = true;
  public isError = false;

  ngOnInit(): void {
    this.isLoading = true;
    this.subscription.add(
      this.todoService.todos$.subscribe({
        next: (todosResponse) => {
          this.todos = todosResponse.items;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error fetching todos', err);
          this.isError = true;
        },
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public drop(event: CdkDragDrop<Todo[]>) {
    const originalOrder = [...this.todos];

    moveItemInArray(this.todos, event.previousIndex, event.currentIndex);

    this.todoService.reorderTodos(this.todos).subscribe({
      next: () => {
        this.isLoading = true;
        this.todoService.triggerRefresh();
      },
      error: () => {
        console.error('Error reordering todos');
        this.todos = originalOrder;
      },
    });
  }
}
