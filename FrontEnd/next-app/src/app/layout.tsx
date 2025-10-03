import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import '@/app/index.module.css'

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="p-0 m-0">
        {children}
      </body>
    </html>
  );
}
