import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoCard } from './todo-card';

describe('Draggable', () => {
  let component: TodoCard;
  let fixture: ComponentFixture<TodoCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoCard],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
