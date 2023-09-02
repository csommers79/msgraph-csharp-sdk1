import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AngularEmailsComponent } from './angular-emails.component';

describe('AngularEmailsComponent', () => {
  let component: AngularEmailsComponent;
  let fixture: ComponentFixture<AngularEmailsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AngularEmailsComponent]
    });
    fixture = TestBed.createComponent(AngularEmailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
