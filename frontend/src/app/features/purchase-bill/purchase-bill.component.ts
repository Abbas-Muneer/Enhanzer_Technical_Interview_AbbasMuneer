import { CommonModule } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { startWith } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { LocationService } from '../../services/location.service';
import { Location } from '../../models/location.model';
import { PurchaseBillItem } from '../../models/purchase-bill-item.model';

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './purchase-bill.component.html',
  styleUrl: './purchase-bill.component.css'
})
export class PurchaseBillComponent {
  private readonly fb = inject(FormBuilder);
  private readonly locationService = inject(LocationService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly items = signal<PurchaseBillItem[]>([]);
  protected readonly locations = signal<Location[]>([]);
  protected readonly filteredItems = signal<string[]>([]);
  protected readonly loadingLocations = signal(true);
  protected readonly locationError = signal('');

  protected readonly catalogItems = ['Mango', 'Apple', 'Banana', 'Orange', 'Grapes', 'Kiwi', 'Strawberry'];

  protected readonly entryForm = this.fb.group({
    item: ['', [Validators.required]],
    batch: ['', [Validators.required]],
    standardCost: [0, [Validators.required, Validators.min(0)]],
    standardPrice: [0, [Validators.required, Validators.min(0)]],
    margin: [{ value: 0, disabled: true }, [Validators.required]],
    qty: [1, [Validators.required, Validators.min(1)]],
    freeQty: [0, [Validators.min(0)]],
    discount: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    totalCost: [{ value: 0, disabled: true }],
    totalSelling: [{ value: 0, disabled: true }]
  });

  protected readonly totalItems = computed(() => this.items().length);
  protected readonly totalQuantity = computed(() => this.items().reduce((sum, item) => sum + item.qty, 0));
  protected readonly totalFreeQuantity = computed(() => this.items().reduce((sum, item) => sum + item.freeQty, 0));
  protected readonly grossTotal = computed(() => this.items().reduce((sum, item) => sum + (item.standardCost * item.qty), 0));
  protected readonly totalDiscountAmount = computed(() => this.grossTotal() - this.items().reduce((sum, item) => sum + item.totalCost, 0));
  protected readonly netTotal = computed(() => this.items().reduce((sum, item) => sum + item.totalCost, 0));
  protected readonly totalSellingSum = computed(() => this.items().reduce((sum, item) => sum + item.totalSelling, 0));

  constructor() {
    this.loadLocations();
    this.setupFormRecalculation();
    this.setupAutocomplete();
  }

  protected addRow(): void {
    if (this.entryForm.invalid) {
      this.entryForm.markAllAsTouched();
      return;
    }

    const rawValue = this.entryForm.getRawValue();
    const row: PurchaseBillItem = {
      item: (rawValue.item ?? '').trim(),
      batch: rawValue.batch ?? '',
      standardCost: Number(rawValue.standardCost),
      standardPrice: Number(rawValue.standardPrice),
      margin: Number(rawValue.margin),
      qty: Number(rawValue.qty),
      freeQty: Number(rawValue.freeQty),
      discount: Number(rawValue.discount),
      totalCost: Number(rawValue.totalCost),
      totalSelling: Number(rawValue.totalSelling)
    };

    this.items.update((currentRows) => [...currentRows, row]);

    this.entryForm.patchValue({
      item: '',
      standardCost: 0,
      standardPrice: 0,
      qty: 1,
      freeQty: 0,
      discount: 0
    });
    this.filteredItems.set([]);
    this.recalculateValues();
  }

  protected logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  protected hasError(controlName: string, errorKey: string): boolean {
    const control = this.entryForm.get(controlName);
    return !!control && control.touched && control.hasError(errorKey);
  }

  protected selectSuggestion(itemName: string): void {
    this.entryForm.patchValue({ item: itemName });
    this.filteredItems.set([]);
  }

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  }

  private loadLocations(): void {
    this.locationService.getLocations()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (locations) => {
          this.locations.set(locations);
          this.loadingLocations.set(false);
        },
        error: () => {
          this.locationError.set('Unable to load batches. Login again to refresh locations.');
          this.loadingLocations.set(false);
        }
      });
  }

  private setupFormRecalculation(): void {
    this.entryForm.valueChanges
      .pipe(
        startWith(this.entryForm.getRawValue()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.recalculateValues());
  }

  private setupAutocomplete(): void {
    this.entryForm.get('item')?.valueChanges
      .pipe(
        startWith(''),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((value) => {
        const term = (value ?? '').toString().trim().toLowerCase();
        if (!term) {
          this.filteredItems.set([]);
          return;
        }

        this.filteredItems.set(
          this.catalogItems.filter((item) => item.toLowerCase().includes(term))
        );
      });
  }

  private recalculateValues(): void {
    const standardCost = Number(this.entryForm.get('standardCost')?.value ?? 0);
    const standardPrice = Number(this.entryForm.get('standardPrice')?.value ?? 0);
    const qty = Number(this.entryForm.get('qty')?.value ?? 0);
    const discount = Number(this.entryForm.get('discount')?.value ?? 0);

    const margin = standardPrice - standardCost;
    const subtotalCost = standardCost * qty;
    const totalCost = subtotalCost - (subtotalCost * discount / 100);
    const totalSelling = standardPrice * qty;

    this.entryForm.patchValue({
      margin: Number.isFinite(margin) ? margin : 0,
      totalCost: Number.isFinite(totalCost) ? totalCost : 0,
      totalSelling: Number.isFinite(totalSelling) ? totalSelling : 0
    }, { emitEvent: false });
  }
}
