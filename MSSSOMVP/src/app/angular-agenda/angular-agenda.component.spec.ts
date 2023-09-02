import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AngularAgendaComponent } from './angular-agenda.component';

describe('AngularAgendaComponent', () => {
  let component: AngularAgendaComponent;
  let fixture: ComponentFixture<AngularAgendaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AngularAgendaComponent]
    });
    fixture = TestBed.createComponent(AngularAgendaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
