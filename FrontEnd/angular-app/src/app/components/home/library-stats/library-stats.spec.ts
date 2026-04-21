import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LibraryStats } from './library-stats';

describe('LibraryStats', () => {
  let component: LibraryStats;
  let fixture: ComponentFixture<LibraryStats>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LibraryStats],
    }).compileComponents();

    fixture = TestBed.createComponent(LibraryStats);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
