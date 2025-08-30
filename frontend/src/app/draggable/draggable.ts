import { Component, input } from '@angular/core';
import { CdkDrag } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-draggable',
  imports: [CdkDrag],
  templateUrl: './draggable.html',
  styleUrl: './draggable.css',
})
export class Draggable {
  public todoDate = input.required<Date, string>({
    transform: (value: string): Date => new Date(Number(value)),
  });
}
