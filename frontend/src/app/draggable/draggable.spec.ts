import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Draggable } from './draggable';

describe('Draggable', () => {
  let component: Draggable;
  let fixture: ComponentFixture<Draggable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Draggable]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Draggable);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
