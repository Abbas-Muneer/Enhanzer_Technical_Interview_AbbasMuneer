import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { PurchaseBillComponent } from './purchase-bill.component';
import { LocationService } from '../../services/location.service';
import { AuthService } from '../../services/auth.service';

describe('PurchaseBillComponent', () => {
  let component: PurchaseBillComponent;
  let fixture: ComponentFixture<PurchaseBillComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseBillComponent],
      providers: [
        provideRouter([]),
        {
          provide: LocationService,
          useValue: {
            getLocations: () => of([
              { id: 1, locationCode: 'B001', locationName: 'BATCH001' },
              { id: 2, locationCode: 'B002', locationName: 'BATCH002' }
            ])
          }
        },
        {
          provide: AuthService,
          useValue: {
            logout: jasmine.createSpy('logout')
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PurchaseBillComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('calculates margin, total cost, and total selling live', () => {
    component['entryForm'].patchValue({
      standardCost: 100,
      standardPrice: 150,
      qty: 5,
      discount: 20
    });

    expect(component['entryForm'].get('margin')?.value).toBe(50);
    expect(component['entryForm'].get('totalCost')?.value).toBe(400);
    expect(component['entryForm'].get('totalSelling')?.value).toBe(750);
  });

  it('adds a row when the form is valid', () => {
    component['entryForm'].patchValue({
      item: 'Mango',
      batch: 'BATCH001',
      standardCost: 100,
      standardPrice: 150,
      qty: 5,
      freeQty: 1,
      discount: 20
    });

    component['addRow']();

    expect(component['items']().length).toBe(1);
    expect(component['items']()[0].item).toBe('Mango');
    expect(component['entryForm'].get('item')?.value).toBe('');
  });

  it('updates summary totals after rows are added', () => {
    component['entryForm'].patchValue({
      item: 'Apple',
      batch: 'BATCH001',
      standardCost: 50,
      standardPrice: 80,
      qty: 4,
      freeQty: 0,
      discount: 10
    });
    component['addRow']();

    component['entryForm'].patchValue({
      item: 'Banana',
      batch: 'BATCH002',
      standardCost: 40,
      standardPrice: 60,
      qty: 6,
      freeQty: 2,
      discount: 0
    });
    component['addRow']();

    expect(component['totalItems']()).toBe(2);
    expect(component['totalQuantity']()).toBe(10);
    expect(component['netTotal']()).toBe(420);
  });
});
