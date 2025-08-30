import { Component, input } from '@angular/core';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { DeleteButton } from '../delete-button/delete-button';
import { Todo } from '../../types/api/todo.model';

@Component({
  selector: 'app-draggable',
  imports: [CdkDrag, DeleteButton],
  templateUrl: './draggable.html',
  styleUrl: './draggable.css',
})
export class Draggable {
  public todo = input.required<Todo>();
}
