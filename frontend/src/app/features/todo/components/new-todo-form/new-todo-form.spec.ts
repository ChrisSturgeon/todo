import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { TodoService } from '../../services/todo-service/todo.service';
import { NewTodoForm } from './new-todo-form';
import type { TodoResponse } from '@api-types/todo/todo.types';

describe('NewTodoForm', () => {
  let component: NewTodoForm;
  let fixture: ComponentFixture<NewTodoForm>;
  let mockTodoService: jasmine.SpyObj<TodoService>;

  beforeEach(async () => {
    mockTodoService = jasmine.createSpyObj('TodoService', [
      'createTodo',
      'triggerRefresh',
    ]);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, NewTodoForm],
      providers: [{ provide: TodoService, useValue: mockTodoService }],
    }).compileComponents();

    fixture = TestBed.createComponent(NewTodoForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('form validation', () => {
    it('should be invalid when empty', () => {
      expect(component.newTodoForm.valid).toBeFalse();
    });

    it('should require a minimum of 3 characters', () => {
      component.name?.setValue('ab');
      expect(component.name?.valid).toBeFalse();
      component.name?.setValue('abc');
      expect(component.name?.valid).toBeTrue();
    });

    it('should not allow more than 50 characters', () => {
      component.name?.setValue('a'.repeat(51));
      expect(component.name?.valid).toBeFalse();
    });
  });

  describe('markup', () => {
    it('should render in an HTML section element', () => {
      const section: HTMLElement =
        fixture.nativeElement.querySelector('section');
      expect(section).toBeTruthy();
    });

    it('should have a section labelled by the heading', () => {
      const heading: HTMLHeadingElement =
        fixture.nativeElement.querySelector('h2');

      const section: HTMLElement =
        fixture.nativeElement.querySelector('section');

      expect(section?.getAttribute('aria-labelledby')).toEqual(
        heading?.getAttribute('id')
      );
    });

    it('should have a semantic link between the input and label', () => {
      const label: HTMLLabelElement =
        fixture.nativeElement.querySelector('label');

      const input: HTMLInputElement =
        fixture.nativeElement.querySelector('input');

      expect(label?.getAttribute('for')).toEqual(input?.getAttribute('id'));
    });
  });

  describe('submitNewTodo', () => {
    beforeEach(() => {
      const fakeTodo: TodoResponse = {
        id: '123',
        name: 'Test todo',
        description: '',
        position: 0,
        completed: false,
        createdAt: '1234',
        updatedAt: '2341',
      };

      mockTodoService.createTodo.and.returnValue(of(fakeTodo));
    });

    it('should call createTodo and triggerRefresh when valid', () => {
      const todoName = 'My test dodo';
      component?.name?.setValue(todoName);

      component.submitNewTodo();

      expect(mockTodoService.createTodo).toHaveBeenCalledWith(todoName);
      expect(mockTodoService.triggerRefresh).toHaveBeenCalled();
    });

    it('should reset the form after successful submission', () => {
      component.name?.setValue('Fake todo');

      component.submitNewTodo();

      expect(component.newTodoForm.value.name).toBeNull();
    });

    it('should not call createTodo when todo name is invalid', () => {
      component.name?.setValue('');

      component.submitNewTodo();

      expect(mockTodoService.createTodo).not.toHaveBeenCalled();
    });
  });

  describe('template valiation messages', () => {
    const testCases = [
      {
        value: '',
        expectedMessage: 'Name is required',
        errorKey: 'required',
      },
      {
        value: 'ab',
        expectedMessage: 'Name must be at least 3 characters',
        errorKey: 'minlength',
      },
      {
        value: 'a'.repeat(51),
        expectedMessage: 'Name must be 50 characters or less',
        errorKey: 'maxlength',
      },
    ];

    testCases.forEach(({ value, expectedMessage, errorKey }) => {
      it(`should show ${expectedMessage} when input is ${value}`, () => {
        const input: HTMLInputElement =
          fixture.nativeElement.querySelector('#name');

        input.value = value;
        input.dispatchEvent(new Event('input'));

        component.name?.markAsTouched();
        fixture.detectChanges();

        const alertText =
          fixture.nativeElement.querySelector('.alert p')?.textContent;
        expect(alertText).toContain(expectedMessage);
      });
    });
  });
});
