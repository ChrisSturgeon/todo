import { Todo } from '../../types/api/todo.model';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { TodoService } from '../todos/todo';
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
  imports: [Draggable, CdkDropList, AddButton, NewTodoForm],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy {
  private todoService: TodoService = inject(TodoService);
  private subscription = new Subscription();

  public todos: Todo[] = [];

  ngOnInit(): void {
    this.subscription.add(
      this.todoService.refresh$
        .pipe(
          startWith(undefined),
          switchMap(() => this.todoService.fetchTodos())
        )
        .subscribe((todos) => {
          this.todos = todos;
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
    });
  }
}
