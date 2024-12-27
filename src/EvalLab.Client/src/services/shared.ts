export type Page<T> = {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  items: T[];
};

export type Result<T, E = Error> =
  | { succeeded: true; failed: false; value: T }
  | { succeeded: false; failed: true; error: E };

export const result = Object.freeze({
  success<T>(value: T): Result<T> {
    return { succeeded: true, failed: false, value };
  },
  failure<E>(error: E): Result<never, E> {
    return { succeeded: false, failed: true, error };
  },
});

export type Entity = {
  id: string;
  name: string;
  createdDate: Date;
  updatedDate: Date;
};

export type GetAllParams = {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  additionalParams?: Record<string, string>;
};

export type GenericService<NewT, T extends Entity> = {
  create(item: NewT): Promise<Result<T>>;
  getAll(params: GetAllParams): Promise<Result<Page<T>>>;
  get(id: string): Promise<Result<T>>;
  delete(id: string): Promise<Result<true>>;
};

export function createGenericService<NewT, T extends Entity>(
  baseUrl: string,
  mapFn: (data: unknown) => T,
): GenericService<NewT, T> {
  return {
    async create(item: NewT): Promise<Result<T>> {
      const response = await makeRequest<T>(baseUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(item),
      });

      if (response.failed) {
        return response;
      }

      return result.success(mapFn(response.value));
    },

    async getAll({
      pageNumber = 1,
      pageSize = 50,
      sortBy = 'createdDate',
      sortOrder = 'desc',
      additionalParams = {},
    }: GetAllParams): Promise<Result<Page<T>>> {
      const searchParams = new URLSearchParams();
      searchParams.append('pageNumber', pageNumber.toString());
      searchParams.append('pageSize', pageSize.toString());
      searchParams.append('sortBy', sortBy);
      searchParams.append('sortOrder', sortOrder);

      for (const key in additionalParams) {
        searchParams.append(key, additionalParams[key]);
      }

      const response = await makeRequest<Page<T>>(`${baseUrl}?${searchParams.toString()}`, {
        method: 'GET',
      });

      if (response.failed) {
        return response;
      }

      return result.success({
        ...response.value,
        items: response.value.items.map(mapFn),
      });
    },

    async get(id: string): Promise<Result<T>> {
      const response = await makeRequest<T>(`${baseUrl}/${id}`, { method: 'GET' });

      if (response.failed) {
        return response;
      }

      return result.success(mapFn(response.value));
    },

    async delete(id: string): Promise<Result<true>> {
      return makeRequest(`${baseUrl}/${id}`, { method: 'DELETE' });
    },
  };
}

export async function makeRequest<T>(url: string, options: RequestInit): Promise<Result<T>> {
  try {
    const response = await fetch(url, options);

    if (response.ok === false) {
      return result.failure(new Error(`${response.status} ${response.statusText}`));
    }

    if (response.status === 204) {
      return result.success(true as unknown as T);
    }

    const data = await response.json();
    return result.success(data);
  } catch (error) {
    console.error(error);
    return result.failure(new Error('Failed to make request'));
  }
}
