/* eslint-disable */
import { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  /** The `DateTime` scalar represents an ISO-8601 compliant date time type. */
  DateTime: { input: any; output: any; }
};

export type Album = {
  __typename?: 'Album';
  artists: Array<SimplifiedArtist>;
  id: Scalars['String']['output'];
  imageUrl: Scalars['String']['output'];
  name: Scalars['String']['output'];
  releaseDate: Scalars['String']['output'];
  tracks: Array<Track>;
};

/** Defines when a policy shall be executed. */
export enum ApplyPolicy {
  /** After the resolver was executed. */
  AfterResolver = 'AFTER_RESOLVER',
  /** Before the resolver was executed. */
  BeforeResolver = 'BEFORE_RESOLVER',
  /** The policy is applied in the validation step before the execution. */
  Validation = 'VALIDATION'
}

export type Artist = {
  __typename?: 'Artist';
  id: Scalars['String']['output'];
  imageUrl: Scalars['String']['output'];
  name: Scalars['String']['output'];
};

export type ArtistByAlbum = {
  __typename?: 'ArtistByAlbum';
  artist: Artist;
  artistId: Scalars['String']['output'];
};

export type ArtistByAlbumFilterInput = {
  and?: InputMaybe<Array<ArtistByAlbumFilterInput>>;
  artistId?: InputMaybe<StringOperationFilterInput>;
  or?: InputMaybe<Array<ArtistByAlbumFilterInput>>;
};

export type ArtistByTrack = {
  __typename?: 'ArtistByTrack';
  artist: Artist;
  artistId: Scalars['String']['output'];
};

export type ArtistByTrackFilterInput = {
  and?: InputMaybe<Array<ArtistByTrackFilterInput>>;
  artistId?: InputMaybe<StringOperationFilterInput>;
  or?: InputMaybe<Array<ArtistByTrackFilterInput>>;
};

export type DateTimeOperationFilterInput = {
  eq?: InputMaybe<Scalars['DateTime']['input']>;
  gt?: InputMaybe<Scalars['DateTime']['input']>;
  gte?: InputMaybe<Scalars['DateTime']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['DateTime']['input']>>>;
  lt?: InputMaybe<Scalars['DateTime']['input']>;
  lte?: InputMaybe<Scalars['DateTime']['input']>;
  neq?: InputMaybe<Scalars['DateTime']['input']>;
  ngt?: InputMaybe<Scalars['DateTime']['input']>;
  ngte?: InputMaybe<Scalars['DateTime']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['DateTime']['input']>>>;
  nlt?: InputMaybe<Scalars['DateTime']['input']>;
  nlte?: InputMaybe<Scalars['DateTime']['input']>;
};

export type Friend = {
  __typename?: 'Friend';
  displayName?: Maybe<Scalars['String']['output']>;
  id: Scalars['String']['output'];
  imageUrl?: Maybe<Scalars['String']['output']>;
};

export type IntOperationFilterInput = {
  eq?: InputMaybe<Scalars['Int']['input']>;
  gt?: InputMaybe<Scalars['Int']['input']>;
  gte?: InputMaybe<Scalars['Int']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['Int']['input']>>>;
  lt?: InputMaybe<Scalars['Int']['input']>;
  lte?: InputMaybe<Scalars['Int']['input']>;
  neq?: InputMaybe<Scalars['Int']['input']>;
  ngt?: InputMaybe<Scalars['Int']['input']>;
  ngte?: InputMaybe<Scalars['Int']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['Int']['input']>>>;
  nlt?: InputMaybe<Scalars['Int']['input']>;
  nlte?: InputMaybe<Scalars['Int']['input']>;
};

export type Me = {
  __typename?: 'Me';
  displayName?: Maybe<Scalars['String']['output']>;
  friends: Array<Friend>;
  id: Scalars['String']['output'];
  imageUrl?: Maybe<Scalars['String']['output']>;
};

/** Information about pagination in a connection. */
export type PageInfo = {
  __typename?: 'PageInfo';
  /** When paginating forwards, the cursor to continue. */
  endCursor?: Maybe<Scalars['String']['output']>;
  /** Indicates whether more edges exist following the set defined by the clients arguments. */
  hasNextPage: Scalars['Boolean']['output'];
  /** Indicates whether more edges exist prior the set defined by the clients arguments. */
  hasPreviousPage: Scalars['Boolean']['output'];
  /** When paginating backwards, the cursor to continue. */
  startCursor?: Maybe<Scalars['String']['output']>;
};

export type Play = {
  __typename?: 'Play';
  timeOfPlay: Scalars['DateTime']['output'];
  track: Track;
  trackId: Scalars['String']['output'];
};

export type PlayFilterInput = {
  and?: InputMaybe<Array<PlayFilterInput>>;
  or?: InputMaybe<Array<PlayFilterInput>>;
  timeOfPlay?: InputMaybe<DateTimeOperationFilterInput>;
  trackId?: InputMaybe<StringOperationFilterInput>;
};

/** A connection to a list of items. */
export type PlaysConnection = {
  __typename?: 'PlaysConnection';
  /** A list of edges. */
  edges?: Maybe<Array<PlaysEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Play>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type PlaysEdge = {
  __typename?: 'PlaysEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: Play;
};

export type Query = {
  __typename?: 'Query';
  album: Album;
  artist: Artist;
  artistsByAlbum: Array<ArtistByAlbum>;
  artistsByTrack: Array<ArtistByTrack>;
  me?: Maybe<Me>;
  plays?: Maybe<PlaysConnection>;
  topAlbums?: Maybe<TopAlbumsConnection>;
  topArtists?: Maybe<TopArtistsConnection>;
  topSongs?: Maybe<TopSongsConnection>;
  track: Track;
};


export type QueryAlbumArgs = {
  albumId: Scalars['String']['input'];
};


export type QueryArtistArgs = {
  artistId: Scalars['String']['input'];
};


export type QueryArtistsByAlbumArgs = {
  albumId: Scalars['String']['input'];
  where?: InputMaybe<ArtistByAlbumFilterInput>;
};


export type QueryArtistsByTrackArgs = {
  trackId: Scalars['String']['input'];
  where?: InputMaybe<ArtistByTrackFilterInput>;
};


export type QueryPlaysArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  from?: InputMaybe<Scalars['DateTime']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
  to?: InputMaybe<Scalars['DateTime']['input']>;
  userId: Scalars['String']['input'];
  where?: InputMaybe<PlayFilterInput>;
};


export type QueryTopAlbumsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  artistId?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  from?: InputMaybe<Scalars['DateTime']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
  to?: InputMaybe<Scalars['DateTime']['input']>;
  userId: Scalars['String']['input'];
  where?: InputMaybe<TopAlbumFilterInput>;
};


export type QueryTopArtistsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  from?: InputMaybe<Scalars['DateTime']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
  to?: InputMaybe<Scalars['DateTime']['input']>;
  userId: Scalars['String']['input'];
  where?: InputMaybe<TopArtistFilterInput>;
};


export type QueryTopSongsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  albumId?: InputMaybe<Scalars['String']['input']>;
  artistId?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  from?: InputMaybe<Scalars['DateTime']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
  to?: InputMaybe<Scalars['DateTime']['input']>;
  userId: Scalars['String']['input'];
  where?: InputMaybe<TopSongFilterInput>;
};


export type QueryTrackArgs = {
  trackId: Scalars['String']['input'];
};

export type SimplifiedAlbum = {
  __typename?: 'SimplifiedAlbum';
  id: Scalars['String']['output'];
  imageUrl: Scalars['String']['output'];
  name: Scalars['String']['output'];
};

export type SimplifiedArtist = {
  __typename?: 'SimplifiedArtist';
  id: Scalars['String']['output'];
  name: Scalars['String']['output'];
};

export type StringOperationFilterInput = {
  and?: InputMaybe<Array<StringOperationFilterInput>>;
  contains?: InputMaybe<Scalars['String']['input']>;
  endsWith?: InputMaybe<Scalars['String']['input']>;
  eq?: InputMaybe<Scalars['String']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
  ncontains?: InputMaybe<Scalars['String']['input']>;
  nendsWith?: InputMaybe<Scalars['String']['input']>;
  neq?: InputMaybe<Scalars['String']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
  nstartsWith?: InputMaybe<Scalars['String']['input']>;
  or?: InputMaybe<Array<StringOperationFilterInput>>;
  startsWith?: InputMaybe<Scalars['String']['input']>;
};

export type TopAlbum = {
  __typename?: 'TopAlbum';
  album: Album;
  albumId: Scalars['String']['output'];
  playCount: Scalars['Int']['output'];
};

export type TopAlbumFilterInput = {
  albumId?: InputMaybe<StringOperationFilterInput>;
  and?: InputMaybe<Array<TopAlbumFilterInput>>;
  or?: InputMaybe<Array<TopAlbumFilterInput>>;
  playCount?: InputMaybe<IntOperationFilterInput>;
};

/** A connection to a list of items. */
export type TopAlbumsConnection = {
  __typename?: 'TopAlbumsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<TopAlbumsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<TopAlbum>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type TopAlbumsEdge = {
  __typename?: 'TopAlbumsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: TopAlbum;
};

export type TopArtist = {
  __typename?: 'TopArtist';
  artist: Artist;
  artistId: Scalars['String']['output'];
  playCount: Scalars['Int']['output'];
};

export type TopArtistFilterInput = {
  and?: InputMaybe<Array<TopArtistFilterInput>>;
  artistId?: InputMaybe<StringOperationFilterInput>;
  or?: InputMaybe<Array<TopArtistFilterInput>>;
  playCount?: InputMaybe<IntOperationFilterInput>;
};

/** A connection to a list of items. */
export type TopArtistsConnection = {
  __typename?: 'TopArtistsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<TopArtistsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<TopArtist>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type TopArtistsEdge = {
  __typename?: 'TopArtistsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: TopArtist;
};

export type TopSong = {
  __typename?: 'TopSong';
  playCount: Scalars['Int']['output'];
  track: Track;
  trackId: Scalars['String']['output'];
};

export type TopSongFilterInput = {
  and?: InputMaybe<Array<TopSongFilterInput>>;
  or?: InputMaybe<Array<TopSongFilterInput>>;
  playCount?: InputMaybe<IntOperationFilterInput>;
  trackId?: InputMaybe<StringOperationFilterInput>;
};

/** A connection to a list of items. */
export type TopSongsConnection = {
  __typename?: 'TopSongsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<TopSongsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<TopSong>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type TopSongsEdge = {
  __typename?: 'TopSongsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: TopSong;
};

export type Track = {
  __typename?: 'Track';
  album: SimplifiedAlbum;
  albumArtUrl: Scalars['String']['output'];
  artists: Array<SimplifiedArtist>;
  id: Scalars['String']['output'];
  name: Scalars['String']['output'];
  playCount: Scalars['Int']['output'];
  trackNumber: Scalars['Int']['output'];
};


export type TrackPlayCountArgs = {
  from?: InputMaybe<Scalars['DateTime']['input']>;
  to?: InputMaybe<Scalars['DateTime']['input']>;
  userId: Scalars['String']['input'];
};

export type Get1PlayQueryVariables = Exact<{
  userId: Scalars['String']['input'];
}>;


export type Get1PlayQuery = { __typename?: 'Query', plays?: { __typename?: 'PlaysConnection', edges?: Array<{ __typename?: 'PlaysEdge', cursor: string, node: { __typename?: 'Play', trackId: string } }> | null } | null };

export type AlbumNameAndPictureQueryVariables = Exact<{
  albumId: Scalars['String']['input'];
}>;


export type AlbumNameAndPictureQuery = { __typename?: 'Query', album: { __typename?: 'Album', id: string, name: string, imageUrl: string } };

export type GetAlbumsForTrackQueryVariables = Exact<{
  trackId: Scalars['String']['input'];
}>;


export type GetAlbumsForTrackQuery = { __typename?: 'Query', track: { __typename?: 'Track', id: string, album: { __typename?: 'SimplifiedAlbum', id: string, name: string, imageUrl: string } } };

export type GetArtistsForAlbumQueryVariables = Exact<{
  albumId: Scalars['String']['input'];
}>;


export type GetArtistsForAlbumQuery = { __typename?: 'Query', artistsByAlbum: Array<{ __typename?: 'ArtistByAlbum', artistId: string, artist: { __typename?: 'Artist', id: string, name: string, imageUrl: string } }> };

export type GetTopSongsByAlbumQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  albumId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
}>;


export type GetTopSongsByAlbumQuery = { __typename?: 'Query', topSongs?: { __typename?: 'TopSongsConnection', edges?: Array<{ __typename?: 'TopSongsEdge', node: { __typename?: 'TopSong', trackId: string, playCount: number } }> | null } | null };

export type GetTrackListQueryVariables = Exact<{
  albumId: Scalars['String']['input'];
}>;


export type GetTrackListQuery = { __typename?: 'Query', album: { __typename?: 'Album', id: string, tracks: Array<{ __typename?: 'Track', id: string, trackNumber: number, name: string }> } };

export type GetMeQueryVariables = Exact<{ [key: string]: never; }>;


export type GetMeQuery = { __typename?: 'Query', me?: { __typename?: 'Me', id: string, displayName?: string | null, imageUrl?: string | null } | null };

export type ArtistNameAndPictureQueryVariables = Exact<{
  artistId: Scalars['String']['input'];
}>;


export type ArtistNameAndPictureQuery = { __typename?: 'Query', artist: { __typename?: 'Artist', name: string, imageUrl: string } };

export type GetTop3AlbumsByArtistQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  artistId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
}>;


export type GetTop3AlbumsByArtistQuery = { __typename?: 'Query', topAlbums?: { __typename?: 'TopAlbumsConnection', edges?: Array<{ __typename?: 'TopAlbumsEdge', node: { __typename?: 'TopAlbum', albumId: string, playCount: number, album: { __typename?: 'Album', name: string, imageUrl: string, artists: Array<{ __typename?: 'SimplifiedArtist', name: string }> } } }> | null } | null };

export type GetTop3SongsByArtistQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  artistId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
}>;


export type GetTop3SongsByArtistQuery = { __typename?: 'Query', topSongs?: { __typename?: 'TopSongsConnection', edges?: Array<{ __typename?: 'TopSongsEdge', node: { __typename?: 'TopSong', trackId: string, playCount: number, track: { __typename?: 'Track', name: string, albumArtUrl: string, artists: Array<{ __typename?: 'SimplifiedArtist', name: string }> } } }> | null } | null };

export type GetTop3AlbumsQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
}>;


export type GetTop3AlbumsQuery = { __typename?: 'Query', topAlbums?: { __typename?: 'TopAlbumsConnection', edges?: Array<{ __typename?: 'TopAlbumsEdge', node: { __typename?: 'TopAlbum', albumId: string, playCount: number, album: { __typename?: 'Album', name: string, imageUrl: string, artists: Array<{ __typename?: 'SimplifiedArtist', name: string }> } } }> | null } | null };

export type GetTopArtistQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
}>;


export type GetTopArtistQuery = { __typename?: 'Query', topArtists?: { __typename?: 'TopArtistsConnection', edges?: Array<{ __typename?: 'TopArtistsEdge', node: { __typename?: 'TopArtist', artistId: string, playCount: number, artist: { __typename?: 'Artist', name: string, imageUrl: string } } }> | null } | null };

export type GetTop3SongsQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
}>;


export type GetTop3SongsQuery = { __typename?: 'Query', topSongs?: { __typename?: 'TopSongsConnection', edges?: Array<{ __typename?: 'TopSongsEdge', node: { __typename?: 'TopSong', trackId: string, playCount: number, track: { __typename?: 'Track', name: string, albumArtUrl: string, artists: Array<{ __typename?: 'SimplifiedArtist', name: string }> } } }> | null } | null };

export type GetPlaysQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
  after?: InputMaybe<Scalars['String']['input']>;
}>;


export type GetPlaysQuery = { __typename?: 'Query', plays?: { __typename?: 'PlaysConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean }, edges?: Array<{ __typename?: 'PlaysEdge', cursor: string, node: { __typename?: 'Play', trackId: string, timeOfPlay: any, track: { __typename?: 'Track', name: string, albumArtUrl: string, artists: Array<{ __typename?: 'SimplifiedArtist', name: string }> } } }> | null } | null };

export type GetTopAlbumsQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  artistId?: InputMaybe<Scalars['String']['input']>;
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
  after?: InputMaybe<Scalars['String']['input']>;
}>;


export type GetTopAlbumsQuery = { __typename?: 'Query', topAlbums?: { __typename?: 'TopAlbumsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean }, edges?: Array<{ __typename?: 'TopAlbumsEdge', cursor: string, node: { __typename?: 'TopAlbum', albumId: string, playCount: number, album: { __typename?: 'Album', name: string, imageUrl: string } } }> | null } | null };

export type GetTopArtistsQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
  after?: InputMaybe<Scalars['String']['input']>;
}>;


export type GetTopArtistsQuery = { __typename?: 'Query', topArtists?: { __typename?: 'TopArtistsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean }, edges?: Array<{ __typename?: 'TopArtistsEdge', cursor: string, node: { __typename?: 'TopArtist', artistId: string, playCount: number, artist: { __typename?: 'Artist', name: string, imageUrl: string } } }> | null } | null };

export type GetMeAndFriendsQueryVariables = Exact<{ [key: string]: never; }>;


export type GetMeAndFriendsQuery = { __typename?: 'Query', me?: { __typename?: 'Me', id: string, displayName?: string | null, imageUrl?: string | null, friends: Array<{ __typename?: 'Friend', id: string, displayName?: string | null, imageUrl?: string | null }> } | null };

export type GetTopSongsQueryVariables = Exact<{
  userId: Scalars['String']['input'];
  artistId?: InputMaybe<Scalars['String']['input']>;
  from: Scalars['DateTime']['input'];
  to: Scalars['DateTime']['input'];
  after?: InputMaybe<Scalars['String']['input']>;
}>;


export type GetTopSongsQuery = { __typename?: 'Query', topSongs?: { __typename?: 'TopSongsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean }, edges?: Array<{ __typename?: 'TopSongsEdge', cursor: string, node: { __typename?: 'TopSong', trackId: string, playCount: number, track: { __typename?: 'Track', name: string, albumArtUrl: string, artists: Array<{ __typename?: 'SimplifiedArtist', name: string }> } } }> | null } | null };

export type GetArtistsForTrackQueryVariables = Exact<{
  trackId: Scalars['String']['input'];
}>;


export type GetArtistsForTrackQuery = { __typename?: 'Query', artistsByTrack: Array<{ __typename?: 'ArtistByTrack', artistId: string, artist: { __typename?: 'Artist', id: string, name: string, imageUrl: string } }> };

export type TrackNameAndPictureQueryVariables = Exact<{
  trackId: Scalars['String']['input'];
  userId: Scalars['String']['input'];
  from?: InputMaybe<Scalars['DateTime']['input']>;
  to?: InputMaybe<Scalars['DateTime']['input']>;
}>;


export type TrackNameAndPictureQuery = { __typename?: 'Query', track: { __typename?: 'Track', id: string, name: string, albumArtUrl: string, playCount: number } };


export const Get1PlayDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"Get1Play"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"plays"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"1"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"cursor"}},{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"trackId"}}]}}]}}]}}]}}]} as unknown as DocumentNode<Get1PlayQuery, Get1PlayQueryVariables>;
export const AlbumNameAndPictureDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"AlbumNameAndPicture"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"album"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"albumId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]} as unknown as DocumentNode<AlbumNameAndPictureQuery, AlbumNameAndPictureQueryVariables>;
export const GetAlbumsForTrackDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetAlbumsForTrack"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"trackId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"track"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"trackId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"trackId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"album"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]}}]} as unknown as DocumentNode<GetAlbumsForTrackQuery, GetAlbumsForTrackQueryVariables>;
export const GetArtistsForAlbumDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetArtistsForAlbum"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artistsByAlbum"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"albumId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artistId"}},{"kind":"Field","name":{"kind":"Name","value":"artist"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]}}]} as unknown as DocumentNode<GetArtistsForAlbumQuery, GetArtistsForAlbumQueryVariables>;
export const GetTopSongsByAlbumDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTopSongsByAlbum"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topSongs"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"albumId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"50"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"trackId"}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTopSongsByAlbumQuery, GetTopSongsByAlbumQueryVariables>;
export const GetTrackListDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTrackList"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"album"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"albumId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"albumId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"tracks"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"trackNumber"}},{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}}]}}]} as unknown as DocumentNode<GetTrackListQuery, GetTrackListQueryVariables>;
export const GetMeDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetMe"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"me"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"displayName"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]} as unknown as DocumentNode<GetMeQuery, GetMeQueryVariables>;
export const ArtistNameAndPictureDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"ArtistNameAndPicture"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artist"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"artistId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]} as unknown as DocumentNode<ArtistNameAndPictureQuery, ArtistNameAndPictureQueryVariables>;
export const GetTop3AlbumsByArtistDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTop3AlbumsByArtist"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topAlbums"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"artistId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"5"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"albumId"}},{"kind":"Field","name":{"kind":"Name","value":"album"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"artists"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTop3AlbumsByArtistQuery, GetTop3AlbumsByArtistQueryVariables>;
export const GetTop3SongsByArtistDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTop3SongsByArtist"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topSongs"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"artistId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"5"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"trackId"}},{"kind":"Field","name":{"kind":"Name","value":"track"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"albumArtUrl"}},{"kind":"Field","name":{"kind":"Name","value":"artists"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTop3SongsByArtistQuery, GetTop3SongsByArtistQueryVariables>;
export const GetTop3AlbumsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTop3Albums"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topAlbums"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"5"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"albumId"}},{"kind":"Field","name":{"kind":"Name","value":"album"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"artists"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTop3AlbumsQuery, GetTop3AlbumsQueryVariables>;
export const GetTopArtistDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTopArtist"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topArtists"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"1"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artistId"}},{"kind":"Field","name":{"kind":"Name","value":"artist"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTopArtistQuery, GetTopArtistQueryVariables>;
export const GetTop3SongsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTop3Songs"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topSongs"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"5"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"trackId"}},{"kind":"Field","name":{"kind":"Name","value":"track"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"albumArtUrl"}},{"kind":"Field","name":{"kind":"Name","value":"artists"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTop3SongsQuery, GetTop3SongsQueryVariables>;
export const GetPlaysDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetPlays"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"after"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"plays"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"after"},"value":{"kind":"Variable","name":{"kind":"Name","value":"after"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"20"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"pageInfo"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}},{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"cursor"}},{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"trackId"}},{"kind":"Field","name":{"kind":"Name","value":"track"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"albumArtUrl"}},{"kind":"Field","name":{"kind":"Name","value":"artists"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"timeOfPlay"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetPlaysQuery, GetPlaysQueryVariables>;
export const GetTopAlbumsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTopAlbums"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"after"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topAlbums"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"artistId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"after"},"value":{"kind":"Variable","name":{"kind":"Name","value":"after"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"20"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"pageInfo"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}},{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"cursor"}},{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"albumId"}},{"kind":"Field","name":{"kind":"Name","value":"album"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTopAlbumsQuery, GetTopAlbumsQueryVariables>;
export const GetTopArtistsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTopArtists"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"after"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topArtists"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"after"},"value":{"kind":"Variable","name":{"kind":"Name","value":"after"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"20"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"pageInfo"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}},{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"cursor"}},{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artistId"}},{"kind":"Field","name":{"kind":"Name","value":"artist"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTopArtistsQuery, GetTopArtistsQueryVariables>;
export const GetMeAndFriendsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetMeAndFriends"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"me"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"displayName"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"friends"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"displayName"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]}}]} as unknown as DocumentNode<GetMeAndFriendsQuery, GetMeAndFriendsQueryVariables>;
export const GetTopSongsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTopSongs"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"after"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"topSongs"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"artistId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"artistId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}},{"kind":"Argument","name":{"kind":"Name","value":"after"},"value":{"kind":"Variable","name":{"kind":"Name","value":"after"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"IntValue","value":"20"}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"pageInfo"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}},{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"cursor"}},{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"trackId"}},{"kind":"Field","name":{"kind":"Name","value":"track"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"albumArtUrl"}},{"kind":"Field","name":{"kind":"Name","value":"artists"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"playCount"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTopSongsQuery, GetTopSongsQueryVariables>;
export const GetArtistsForTrackDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetArtistsForTrack"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"trackId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artistsByTrack"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"trackId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"trackId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"artistId"}},{"kind":"Field","name":{"kind":"Name","value":"artist"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"imageUrl"}}]}}]}}]}}]} as unknown as DocumentNode<GetArtistsForTrackQuery, GetArtistsForTrackQueryVariables>;
export const TrackNameAndPictureDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"TrackNameAndPicture"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"trackId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"userId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"from"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"to"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"DateTime"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"track"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"trackId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"trackId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"albumArtUrl"}},{"kind":"Field","name":{"kind":"Name","value":"playCount"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"userId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"userId"}}},{"kind":"Argument","name":{"kind":"Name","value":"from"},"value":{"kind":"Variable","name":{"kind":"Name","value":"from"}}},{"kind":"Argument","name":{"kind":"Name","value":"to"},"value":{"kind":"Variable","name":{"kind":"Name","value":"to"}}}]}]}}]}}]} as unknown as DocumentNode<TrackNameAndPictureQuery, TrackNameAndPictureQueryVariables>;