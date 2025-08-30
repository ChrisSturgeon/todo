import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewTodoForm } from './new-todo-form';

describe('NewTodoForm', () => {
  let component: NewTodoForm;
  let fixture: ComponentFixture<NewTodoForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewTodoForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewTodoForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
