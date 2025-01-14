/* eslint-disable */
import * as types from './graphql';
import { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';

/**
 * Map of all GraphQL operations in the project.
 *
 * This map has several performance disadvantages:
 * 1. It is not tree-shakeable, so it will include all operations in the project.
 * 2. It is not minifiable, so the string of a GraphQL query will be multiple times inside the bundle.
 * 3. It does not support dead code elimination, so it will add unused operations.
 *
 * Therefore it is highly recommended to use the babel or swc plugin for production.
 * Learn more about it here: https://the-guild.dev/graphql/codegen/plugins/presets/preset-client#reducing-bundle-size
 */
const documents = {
    "\n  query Get1Play($userId: String!) {\n    plays(userId: $userId, first: 1) {\n      edges {\n        cursor\n        node {\n          trackId\n        }\n      }\n    }\n  }\n": types.Get1PlayDocument,
    "\n  query AlbumNameAndPicture($albumId: String!) {\n    album(albumId: $albumId) {\n      id,\n      name,\n      imageUrl\n    }\n  }\n": types.AlbumNameAndPictureDocument,
    "\n  query GetAlbumsForTrack($trackId: String!) {\n    track(trackId: $trackId) {\n      id\n      album {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n": types.GetAlbumsForTrackDocument,
    "\n  query GetArtistsForAlbum($albumId: String!) {\n    artistsByAlbum(albumId: $albumId) {\n      artistId\n      artist {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n": types.GetArtistsForAlbumDocument,
    "\n  query GetTopSongsByAlbum($userId: String!, $albumId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, albumId: $albumId, from: $from, to: $to, first: 50) {\n      edges {\n        node {\n          trackId\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTopSongsByAlbumDocument,
    "\n  query GetTrackList($albumId: String!) {\n    album(albumId: $albumId) {\n      id\n      tracks {\n        id\n        trackNumber\n        name\n      }\n    }\n  }\n": types.GetTrackListDocument,
    "\n  query GetMe {\n    me {\n      id\n      displayName\n      imageUrl\n    }\n  }": types.GetMeDocument,
    "\n  query ArtistNameAndPicture($artistId: String!) {\n    artist(artistId: $artistId) {\n      name,\n      imageUrl\n    }\n  }\n": types.ArtistNameAndPictureDocument,
    "\n  query GetTop3AlbumsByArtist($userId: String!, $artistId: String!, $from: DateTime!, $to: DateTime!) {\n    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTop3AlbumsByArtistDocument,
    "\n  query GetTop3SongsByArtist($userId: String!, $artistId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, artistId: $artistId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTop3SongsByArtistDocument,
    "\n  query GetTop3Albums($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topAlbums(userId: $userId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTop3AlbumsDocument,
    "\n  query GetTopArtist($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topArtists(userId: $userId, from: $from, to: $to, first: 1) {\n      edges {\n        node {\n          artistId\n          artist {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTopArtistDocument,
    "\n  query GetTop3Songs($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTop3SongsDocument,
    "\n  query GetPlays($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {\n    plays(userId: $userId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          timeOfPlay\n        }\n      }\n    }\n  }\n": types.GetPlaysDocument,
    "\n  query GetTopAlbums($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {\n    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTopAlbumsDocument,
    "\n  query GetTopArtists($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {\n    topArtists(userId: $userId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          artistId\n          artist {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTopArtistsDocument,
    "\n  query GetMeAndFriends {\n    me {\n      id\n      displayName\n      imageUrl\n      friends {\n        id\n        displayName\n        imageUrl\n      }\n    }\n  }": types.GetMeAndFriendsDocument,
    "\n  query GetTopSongs($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {\n    topSongs(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n": types.GetTopSongsDocument,
    "\n  query GetArtistsForTrack($trackId: String!) {\n    artistsByTrack(trackId: $trackId) {\n      artistId\n      artist {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n": types.GetArtistsForTrackDocument,
    "\n  query TrackNameAndPicture($trackId: String!, $userId: String!, $from: DateTime, $to: DateTime) {\n    track(trackId: $trackId) {\n      id,\n      name,\n      albumArtUrl,\n      playCount(userId:$userId, from: $from, to: $to )\n    }\n  }\n": types.TrackNameAndPictureDocument,
};

/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 *
 *
 * @example
 * ```ts
 * const query = gql(`query GetUser($id: ID!) { user(id: $id) { name } }`);
 * ```
 *
 * The query argument is unknown!
 * Please regenerate the types.
 */
export function gql(source: string): unknown;

/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query Get1Play($userId: String!) {\n    plays(userId: $userId, first: 1) {\n      edges {\n        cursor\n        node {\n          trackId\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query Get1Play($userId: String!) {\n    plays(userId: $userId, first: 1) {\n      edges {\n        cursor\n        node {\n          trackId\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query AlbumNameAndPicture($albumId: String!) {\n    album(albumId: $albumId) {\n      id,\n      name,\n      imageUrl\n    }\n  }\n"): (typeof documents)["\n  query AlbumNameAndPicture($albumId: String!) {\n    album(albumId: $albumId) {\n      id,\n      name,\n      imageUrl\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetAlbumsForTrack($trackId: String!) {\n    track(trackId: $trackId) {\n      id\n      album {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetAlbumsForTrack($trackId: String!) {\n    track(trackId: $trackId) {\n      id\n      album {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetArtistsForAlbum($albumId: String!) {\n    artistsByAlbum(albumId: $albumId) {\n      artistId\n      artist {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetArtistsForAlbum($albumId: String!) {\n    artistsByAlbum(albumId: $albumId) {\n      artistId\n      artist {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTopSongsByAlbum($userId: String!, $albumId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, albumId: $albumId, from: $from, to: $to, first: 50) {\n      edges {\n        node {\n          trackId\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTopSongsByAlbum($userId: String!, $albumId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, albumId: $albumId, from: $from, to: $to, first: 50) {\n      edges {\n        node {\n          trackId\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTrackList($albumId: String!) {\n    album(albumId: $albumId) {\n      id\n      tracks {\n        id\n        trackNumber\n        name\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTrackList($albumId: String!) {\n    album(albumId: $albumId) {\n      id\n      tracks {\n        id\n        trackNumber\n        name\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetMe {\n    me {\n      id\n      displayName\n      imageUrl\n    }\n  }"): (typeof documents)["\n  query GetMe {\n    me {\n      id\n      displayName\n      imageUrl\n    }\n  }"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query ArtistNameAndPicture($artistId: String!) {\n    artist(artistId: $artistId) {\n      name,\n      imageUrl\n    }\n  }\n"): (typeof documents)["\n  query ArtistNameAndPicture($artistId: String!) {\n    artist(artistId: $artistId) {\n      name,\n      imageUrl\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTop3AlbumsByArtist($userId: String!, $artistId: String!, $from: DateTime!, $to: DateTime!) {\n    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTop3AlbumsByArtist($userId: String!, $artistId: String!, $from: DateTime!, $to: DateTime!) {\n    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTop3SongsByArtist($userId: String!, $artistId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, artistId: $artistId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTop3SongsByArtist($userId: String!, $artistId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, artistId: $artistId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTop3Albums($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topAlbums(userId: $userId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTop3Albums($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topAlbums(userId: $userId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTopArtist($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topArtists(userId: $userId, from: $from, to: $to, first: 1) {\n      edges {\n        node {\n          artistId\n          artist {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTopArtist($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topArtists(userId: $userId, from: $from, to: $to, first: 1) {\n      edges {\n        node {\n          artistId\n          artist {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTop3Songs($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTop3Songs($userId: String!, $from: DateTime!, $to: DateTime!) {\n    topSongs(userId: $userId, from: $from, to: $to, first: 5) {\n      edges {\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetPlays($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {\n    plays(userId: $userId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          timeOfPlay\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetPlays($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {\n    plays(userId: $userId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          timeOfPlay\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTopAlbums($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {\n    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTopAlbums($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {\n    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          albumId\n          album {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTopArtists($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {\n    topArtists(userId: $userId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          artistId\n          artist {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTopArtists($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {\n    topArtists(userId: $userId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          artistId\n          artist {\n            name\n            imageUrl\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetMeAndFriends {\n    me {\n      id\n      displayName\n      imageUrl\n      friends {\n        id\n        displayName\n        imageUrl\n      }\n    }\n  }"): (typeof documents)["\n  query GetMeAndFriends {\n    me {\n      id\n      displayName\n      imageUrl\n      friends {\n        id\n        displayName\n        imageUrl\n      }\n    }\n  }"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetTopSongs($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {\n    topSongs(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetTopSongs($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {\n    topSongs(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {\n      pageInfo {\n        hasNextPage\n      }\n      edges {\n        cursor\n        node {\n          trackId\n          track {\n            name\n            albumArtUrl\n            artists {\n              name\n            }\n          }\n          playCount\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetArtistsForTrack($trackId: String!) {\n    artistsByTrack(trackId: $trackId) {\n      artistId\n      artist {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetArtistsForTrack($trackId: String!) {\n    artistsByTrack(trackId: $trackId) {\n      artistId\n      artist {\n        id\n        name\n        imageUrl\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query TrackNameAndPicture($trackId: String!, $userId: String!, $from: DateTime, $to: DateTime) {\n    track(trackId: $trackId) {\n      id,\n      name,\n      albumArtUrl,\n      playCount(userId:$userId, from: $from, to: $to )\n    }\n  }\n"): (typeof documents)["\n  query TrackNameAndPicture($trackId: String!, $userId: String!, $from: DateTime, $to: DateTime) {\n    track(trackId: $trackId) {\n      id,\n      name,\n      albumArtUrl,\n      playCount(userId:$userId, from: $from, to: $to )\n    }\n  }\n"];

export function gql(source: string) {
  return (documents as any)[source] ?? {};
}

export type DocumentType<TDocumentNode extends DocumentNode<any, any>> = TDocumentNode extends DocumentNode<  infer TType,  any>  ? TType  : never;