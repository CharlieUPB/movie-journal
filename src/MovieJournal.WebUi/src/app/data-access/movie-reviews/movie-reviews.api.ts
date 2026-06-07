import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import {
  CreateMovieReviewRequest,
  ListMyMovieReviewsOptions,
  UpdateMovieReviewRequest,
} from '../../domain/movie-reviews/movie-review.requests';
import {
  MovieReviewResponse,
  MovieReviewsListResponse,
} from '../../domain/movie-reviews/movie-review.responses';

@Injectable({ providedIn: 'root' })
export class MovieReviewsApi {
  private readonly api = inject(ApiClient);

  getAll(): Observable<MovieReviewsListResponse> {
    return this.api.get<MovieReviewsListResponse>('/api/movie-reviews');
  }

  getPublished(): Observable<MovieReviewsListResponse> {
    return this.api.get<MovieReviewsListResponse>('/api/movie-reviews/published');
  }

  getMine(options: ListMyMovieReviewsOptions = {}): Observable<MovieReviewsListResponse> {
    const query = options.status ? `?status=${encodeURIComponent(options.status)}` : '';

    return this.api.get<MovieReviewsListResponse>(`/api/movie-reviews/my${query}`);
  }

  getById(id: string): Observable<MovieReviewResponse> {
    return this.api.get<MovieReviewResponse>(`/api/movie-reviews/${id}`);
  }

  create(request: CreateMovieReviewRequest): Observable<MovieReviewResponse> {
    return this.api.post<CreateMovieReviewRequest, MovieReviewResponse>(
      '/api/movie-reviews',
      request,
    );
  }

  update(id: string, request: UpdateMovieReviewRequest): Observable<MovieReviewResponse> {
    return this.api.put<UpdateMovieReviewRequest, MovieReviewResponse>(
      `/api/movie-reviews/${id}`,
      request,
    );
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`/api/movie-reviews/${id}`);
  }

  publish(id: string): Observable<MovieReviewResponse> {
    return this.api.post<null, MovieReviewResponse>(`/api/movie-reviews/${id}/publish`, null);
  }

  archive(id: string): Observable<MovieReviewResponse> {
    return this.api.post<null, MovieReviewResponse>(`/api/movie-reviews/${id}/archive`, null);
  }
}
